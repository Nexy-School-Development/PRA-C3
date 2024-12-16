<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "voetbalapp";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die("Connection failed: " . $conn->connect_error);
}

$sql = "SELECT Id, Name, Points, CreatorId FROM teams";
$result = $conn->query($sql);

$teams = array();
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $teams[] = $row;
    }
} else {
    echo json_encode(["message" => "No teams found"]);
    exit();
}

// Debugging output
error_log("Teams fetched: " . print_r($teams, true));

echo json_encode($teams);

$conn->close();
