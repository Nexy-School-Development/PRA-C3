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

$sql = "SELECT Id, HomeTeamId, AwayTeamId, Team1Score, Team2Score, Starttime, IsFinished, TourneyId FROM matches";
$result = $conn->query($sql);

$matches = array();
if ($result->num_rows > 0) {
    while ($row = $result->fetch_assoc()) {
        $matches[] = $row;
    }
} else {
    echo json_encode(["message" => "No matches found"]);
    exit();
}

echo json_encode($matches);

$conn->close();
