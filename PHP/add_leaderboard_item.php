<?php

require_once "connection.php";

$data = $_POST["data"];
$data = json_decode($data);

$name = $data->name;
$score = $data->score;

try {
    $add_player = "INSERT INTO `Bladiator`(`name`, `score`) VALUES (?, ?)";
    $statement = $connection->prepare($add_player);
    $statement->bind_param("si", $name, $score);
    $statement->execute();
} catch (PDOException $e) {
    echo "Error: " . $e->getMessage();
}
