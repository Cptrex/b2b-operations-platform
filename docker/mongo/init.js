db.createUser({
  user: 'root',
  pwd: 'STRONG_PASSWORD',
  roles: [ { role: 'root', db: 'admin' } ]
});

// create databases and users for services (optional)
// this script runs only on fresh container initialization
