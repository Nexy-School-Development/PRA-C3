const express = require('express');
const mysql = require('mysql2');

const app = express();
const port = 5116;

const db = mysql.createConnection({
  host: 'localhost',
  user: 'root',
  password: '',
  database: 'voetbalapp'
});

db.connect((err) => {
  if (err) {
    console.error('Error connecting to the database:', err);
    return;
  }
  console.log('Connected to the database');
});

app.get('/api/User', (req, res) => {
  const query = 'SELECT Id, Email, IsAdmin, Balance FROM users';
  db.query(query, (err, results) => {
    if (err) {
      console.error('Error fetching users:', err);
      res.status(500).send('Internal Server Error');
      return;
    }
    res.json(results);
  });
});

// Start the server
app.listen(port, () => {
  console.log(`Server running at http://localhost:${port}/`);
});