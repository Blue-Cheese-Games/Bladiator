<?php

require_once "connection.php";

$data = $_POST["data"];
$data = json_decode($data);

$name = $data->name;
$score = $data->score;
$wave = $data->wave;

try {
    $add_player = "INSERT INTO `Bladiator`(`name`, `score`, `wave`) VALUES (?, ?, ?)";
    $statement = $connection->prepare($add_player);
    $statement->bind_param("sii", $name, $score, $wave);
    $statement->execute();
} catch (PDOException $e) {
    echo "Error: " . $e->getMessage();
}
