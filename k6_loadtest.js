import http from 'k6/http';
import { check, group } from 'k6';
import { Trend } from 'k6/metrics';

const ENDPOINTS = {
    authService: 'http://localhost:5257',
    authBusiness: 'http://localhost:8081',
    serviceBusiness: 'http://localhost:8082',
    serviceOrders: 'http://localhost:8083',
    serviceNotify: 'http://localhost:8084',
    serviceSearch: 'http://localhost:8085',
    serviceAnalytics: 'http://localhost:8086',
};

const TOKENS = {
  auth: 'PASTE_AUTH_TOKEN_HERE',
  business: 'PASTE_BUSINESS_TOKEN_HERE',
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

function safeCheck(res, name) {
  const ok = check(res, {
    [`${name} status 2xx`]: (r) => r && r.status >= 200 && r.status < 300,
  });

  if (res && res.timings) {
    reqDuration.add(res.timings.duration, { endpoint: name });
  }

  return ok;
}

function authProfile() {
  const url = `${ENDPOINTS.authService}/api/profile`;
  const res = http.get(url, {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'auth.profile', service: 'auth' },
    headers: { Authorization: `Bearer ${TOKENS.auth}` },
  });
  safeCheck(res, 'auth.profile');
}

function businessList() {
  const url = `${ENDPOINTS.serviceBusiness}/api/businesses`;
  const res = http.get(url, {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'business.list', service: 'business' },
    headers: { Authorization: `Bearer ${TOKENS.business}` },
  });
  safeCheck(res, 'business.list');
}

function businessCreate() {
  const url = `${ENDPOINTS.serviceBusiness}/api/businesses`;
  const payload = {
    name: `LoadTest Co ${Math.random().toString(36).substring(7)}`,
    metadata: { seed: randomInt(1, 10000) },
  };

  const res = http.post(url, JSON.stringify(payload), {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'business.create', service: 'business' },
    headers: {
      Authorization: `Bearer ${TOKENS.business}`,
      'Content-Type': 'application/json',
    },
  });

  safeCheck(res, 'business.create');
}

function ordersCreate() {
  const url = `${ENDPOINTS.serviceOrders}/api/orders`;
  const order = {
    items: [{ sku: 'SKU-' + randomInt(1000, 9999), qty: randomInt(1, 5) }],
    total: randomInt(100, 10000) / 100,
  };

  const res = http.post(url, JSON.stringify(order), {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'orders.create', service: 'orders' },
    headers: {
      Authorization: `Bearer ${TOKENS.business}`,
      'Content-Type': 'application/json',
    },
  });

  safeCheck(res, 'orders.create');
}

function notifySend() {
  const url = `${ENDPOINTS.serviceNotify}/api/notify`;
  const payload = {
    to: 'user+' + randomInt(1, 1000) + '@example.test',
    message: 'Load test notification',
  };

  const res = http.post(url, JSON.stringify(payload), {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'notify.send', service: 'notify' },
    headers: { 'Content-Type': 'application/json' },
  });

  safeCheck(res, 'notify.send');
}

function searchQuery() {
  const q = ['load', 'test', 'order', 'business', 'notification'][randomInt(0, 4)];
  const url = `${ENDPOINTS.serviceSearch}/api/search?q=${encodeURIComponent(q)}`;

  const res = http.get(url, {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'search.query', service: 'search' },
  });

  safeCheck(res, 'search.query');
}

function analyticsSummary() {
  const url = `${ENDPOINTS.serviceAnalytics}/api/metrics/summary`;

  const res = http.get(url, {
    ...DEFAULT_PARAMS,
    tags: { endpoint: 'analytics.summary', service: 'analytics' },
  });

  safeCheck(res, 'analytics.summary');
}

export default function () {
  const chooser = Math.random();

  if (chooser < 0.18) {
    authProfile();
  } else if (chooser < 0.40) {
    if (Math.random() < 0.5) businessList();
    else businessCreate();
  } else if (chooser < 0.70) {
    ordersCreate();
  } else if (chooser < 0.82) {
    notifySend();
  } else if (chooser < 0.94) {
    searchQuery();
  } else {
    analyticsSummary();
  }
}