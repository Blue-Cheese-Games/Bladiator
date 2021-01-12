<?php

require_once "connection.php";

$get_leaderboard = "SELECT `name`, `score` FROM `Bladiator`";
$result = $connection->query($get_leaderboard);

if ($result->num_rows > 0) {
    $arr = [];
    while ($row = mysqli_fetch_assoc($result)) {
        array_push($arr, $row);
    }

    echo json_encode($arr);
} else {
    echo "There are currently no players on the leaderboard.";
}