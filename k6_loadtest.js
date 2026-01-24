import http from 'k6/http';
import { check } from 'k6';
import { Trend } from 'k6/metrics';

const BASE_URLS = {
    serviceBusiness: 'http://localhost:8082',
    serviceOrders: 'http://localhost:8083',
    serviceSearch: 'http://localhost:8085',
};

const TOKENS = {
    s2s: '',
    client: '',
};

const API = {
    businessClient: {
        createBusiness: {
            method: 'POST',
            url: () => `${BASE_URLS.serviceBusiness}/api/v1/client/business`,
            auth: 'client',
            body: () => ({
                businessName: `BT_${Date.now()}_${Math.random().toString(16).slice(2, 10)}`,
            }),
        },
        getBusinessById: {
            method: 'GET',
            url: (p) => `${BASE_URLS.serviceBusiness}/api/v1/client/business/${p.businessId}`,
            auth: 'client',
        },
    },

    businessInternalUsers: {
        createUser: {
            method: 'POST',
            url: () => `${BASE_URLS.serviceBusiness}/api/v1/internal/user`,
            auth: 'client',
            body: () => ({
                login: `user_${Date.now()}_${Math.random().toString(16).slice(2, 10)}`,
                password: "",
                email: `user_${Date.now()}_${Math.random().toString(16).slice(2, 10)}@test.local`,
                name: `name user_${Date.now()}_${Math.random().toString(16).slice(2, 10)}`,
                businessId: '7930d474-fce3-4e18-abdf-7f2d2cc1e416',
            }),
        },
    },

    businessInternalProducts: {
        addProduct: {
            method: 'POST',
            url: (p) => `${BASE_URLS.serviceBusiness}/api/v1/internal/business/${p.businessId}/products`,
            auth: 'client',
            body: () => ({
                productName: 'PUT_PRODUCT_NAME',
                description: 'PUT_DESCRIPTION',
                price: 100.0,
            }),
        },
        updateProduct: {
            method: 'PUT',
            url: (p) =>
                `${BASE_URLS.serviceBusiness}/api/v1/internal/business/${p.businessId}/products/${p.productId}`,
            auth: 'client',
            body: () => ({
                productName: 'PUT_PRODUCT_NAME',
                description: 'PUT_DESCRIPTION',
                price: 120.0,
            }),
        },
        setAvailability: {
            method: 'PATCH',
            url: (p) =>
                `${BASE_URLS.serviceBusiness}/api/v1/internal/business/${p.businessId}/products/${p.productId}/availability`,
            auth: 'client',
            body: () => ({
                isAvailable: true,
            }),
        },
    },

    orders: {
        createOrder: {
            method: 'POST',
            url: () => `${BASE_URLS.serviceOrders}/api/v1/orders`,
            auth: 'client',
            body: () => ({
                businessId: '7930d474-fce3-4e18-abdf-7f2d2cc1e416',
                customerId: 'PUT_CUSTOMER_ID',
                customerName: 'PUT_CUSTOMER_NAME',
                customerEmail: 'PUT_CUSTOMER_EMAIL',
                customerPhone: 'PUT_CUSTOMER_PHONE',
                items: [
                    {
                        productId: '0d7ddd64-a8ec-4033-b5d7-c3e5dead6648',
                        quantity: 1,
                    },
                ],
            }),
        },
        confirm: {
            method: 'POST',
            url: (p) => `${BASE_URLS.serviceOrders}/api/v1/orders/${p.orderId}/confirm`,
            auth: 'client',
        },
        cancel: {
            method: 'POST',
            url: (p) => `${BASE_URLS.serviceOrders}/api/v1/orders/${p.orderId}/cancel`,
            auth: 'client',
        },
        updatePaymentStatus: {
            method: 'PUT',
            url: (p) => `${BASE_URLS.serviceOrders}/api/v1/orders/${p.orderId}/payment-status`,
            auth: 'client',
            body: () => ({
                paymentStatus: 1,
            }),
        },
        updateDeliveryStatus: {
            method: 'PUT',
            url: (p) => `${BASE_URLS.serviceOrders}/api/v1/orders/${p.orderId}/delivery-status`,
            auth: 'client',
            body: () => ({
                deliveryStatus: 1,
            }),
        },
    },

    search: {
        business: {
            method: 'GET',
            url: (q) =>
                `${BASE_URLS.serviceSearch}/api/v1/search/business?name=${encodeURIComponent(q.name)}`,
            auth: 'client',
        },
        user: {
            method: 'GET',
            url: (q) =>
                `${BASE_URLS.serviceSearch}/api/v1/search/user?name=${encodeURIComponent(q.name)}`,
            auth: 'client',
        },
        accountByLogin: {
            method: 'GET',
            url: (q) =>
                `${BASE_URLS.serviceSearch}/api/v1/search/account/login?login=${encodeURIComponent(q.login)}`,
            auth: 'client',
        },
        accountByEmail: {
            method: 'GET',
            url: (q) =>
                `${BASE_URLS.serviceSearch}/api/v1/search/account/email?email=${encodeURIComponent(q.email)}`,
            auth: 'client',
        },
        accountByName: {
            method: 'GET',
            url: (q) =>
                `${BASE_URLS.serviceSearch}/api/v1/search/account/name?name=${encodeURIComponent(q.name)}`,
            auth: 'client',
        },
    },
};

const TARGET_RPS = 1000;
const TEST_DURATION = '5m';
const PREALLOCATED_VUS = 500;
const MAX_VUS = 4000;

const DEFAULT_PARAMS = {
    timeout: '5s',
};

const reqDuration = new Trend('req_duration');

export const options = {
    discardResponseBodies: true,
    scenarios: {
        rps_test: {
            executor: 'constant-arrival-rate',
            rate: TARGET_RPS,
            timeUnit: '1s',
            duration: TEST_DURATION,
            preAllocatedVUs: PREALLOCATED_VUS,
            maxVUs: MAX_VUS,
        },
    },
    thresholds: {
        http_req_failed: ['rate<0.01'],
        http_req_duration: ['p(95)<1000'],
        checks: ['rate>0.99'],
    },
};

function randomInt(min, max) {
    return Math.floor(Math.random() * (max - min + 1)) + min;
}

function randomPick(arr) {
    return arr[randomInt(0, arr.length - 1)];
}

function safeCheck(res, name) {
    const ok = check(res, {
        [`${name} status 2xx`]: (r) => r && r.status >= 200 && r.status < 300,
    });

    if (res && res.timings) {
        reqDuration.add(res.timings.duration, { endpoint: name });
    }

    return ok;
}

function buildHeaders(authType) {
    const headers = {};

    if (authType === 'client') {
        headers.Authorization = `Bearer ${TOKENS.client}`;
    } else if (authType === 's2s') {
        headers.Authorization = `Bearer ${TOKENS.s2s}`;
    }

    return headers;
}

function requestApi(name, def, args = {}) {
    const method = def.method;
    const url = def.url(args.params || args.query || {});
    const auth = def.auth || 'none';

    let payload = null;
    if (typeof def.body === 'function') {
        payload = def.body(args.bodyCtx || {});
    }

    const headers = buildHeaders(auth);
    if (payload && method !== 'GET') {
        headers['Content-Type'] = 'application/json';
    }

    const params = {
        ...DEFAULT_PARAMS,
        tags: { endpoint: name },
        headers,
    };

    let res;
    if (method === 'GET') {
        res = http.get(url, params);
    } else if (method === 'POST') {
        res = http.post(url, payload ? JSON.stringify(payload) : null, params);
    } else if (method === 'PUT') {
        res = http.put(url, payload ? JSON.stringify(payload) : null, params);
    } else if (method === 'PATCH') {
        res = http.patch(url, payload ? JSON.stringify(payload) : null, params);
    } else {
        return;
    }

    safeCheck(res, name);
}

function callRandomSearch() {
    const names = ['Ivan', 'Petr', '123', 'Test', 'Business'];
    const emails = ['a@test.local', 'b@test.local', 'c@test.local'];
    const logins = ['user1', 'user2', 'admin', 'test'];

    const variants = [
        () => requestApi('search.business', API.search.business, { query: { name: randomPick(names) } }),
        () => requestApi('search.user', API.search.user, { query: { name: randomPick(names) } }),
        () => requestApi('search.accountByLogin', API.search.accountByLogin, { query: { login: randomPick(logins) } }),
        () => requestApi('search.accountByEmail', API.search.accountByEmail, { query: { email: randomPick(emails) } }),
        () => requestApi('search.accountByName', API.search.accountByName, { query: { name: randomPick(names) } }),
    ];

    randomPick(variants)();
}

export default function () {
    const chooser = Math.random();


    if (chooser < 0.25) {
        requestApi('businessClient.createBusiness', API.businessClient.createBusiness);
        return;
    }

    if (chooser < 0.40) {
        requestApi('businessInternalUsers.createUser', API.businessInternalUsers.createUser);
        return;
    }

    if (chooser < 0.65) {
        requestApi('businessInternalProducts.addProduct', API.businessInternalProducts.addProduct, {
            params: { businessId: '7930d474-fce3-4e18-abdf-7f2d2cc1e416' },
        });
        return;
    }

    if (chooser < 0.85) {
        requestApi('orders.createOrder', API.orders.createOrder);
        return;
    }

    callRandomSearch();
}