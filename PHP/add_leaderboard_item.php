<?php

require_once "connection.php";

$data = $_POST["data"];
$data = json_decode($data);

$player_name = $data->name;
$player_score = $data->score;

try {
    $add_player = "INSERT INTO `Bladiator`(`player_name`, `player_score`) VALUES (?, ?)";
    $statement = $connection->prepare($add_player);
    $statement->bind_param("si", $player_name, $player_score);
    $statement->execute();
} catch (PDOException $e) {
    echo "Error: " . $e->getMessage();
}
