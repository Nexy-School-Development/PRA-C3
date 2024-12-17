<?php
header('Content-Type: application/json');
header('Access-Control-Allow-Origin: *');
header('Access-Control-Allow-Methods: POST');
header('Access-Control-Allow-Headers: Content-Type');

$servername = "localhost";
$username = "root";
$password = "";
$dbname = "voetbalapp";

$conn = new mysqli($servername, $username, $password, $dbname);

if ($conn->connect_error) {
    die(json_encode(["message" => "Database connection failed", "error" => $conn->connect_error]));
}

$data = json_decode(file_get_contents("php://input"), true);

if (!empty($data)) {
    foreach ($data as $match) {
        $homeTeamId = $match['HomeTeamId'];
        $awayTeamId = $match['AwayTeamId'];
        $team1Score = $match['Team1Score'];
        $team2Score = $match['Team2Score'];
        $starttime = $match['Starttime'];
        $isFinished = $match['IsFinished'];
        $tourneyId = $match['TourneyId'];

        $sql = "INSERT INTO matches (HomeTeamId, AwayTeamId, Team1Score, Team2Score, Starttime, IsFinished, TourneyId)
                VALUES ('$homeTeamId', '$awayTeamId', '$team1Score', '$team2Score', '$starttime', '$isFinished', '$tourneyId')";

        if (!$conn->query($sql)) {
            echo json_encode(["message" => "Error inserting match", "error" => $conn->error]);
            exit();
        }
    }
    echo json_encode(["message" => "All matches inserted successfully"]);
} else {
    echo json_encode(["message" => "No match data received"]);
}

$conn->close();
