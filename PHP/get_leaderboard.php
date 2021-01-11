<?php

require_once "connection.php";

$get_leaderboard = "SELECT `player_name`, `player_score` FROM `Bladiator`";
$result = $connection->query($get_leaderboard);

if ($result->num_rows > 0) {
    echo json_encode(mysqli_fetch_all($result));
} else {
    echo "There are currently no players on the leaderboard.";
}