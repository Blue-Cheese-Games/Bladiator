<?php

$servername = file_get_contents("../db_connection_info/dbServer.dbase");
$username = file_get_contents("../db_connection_info/dbUsername.dbase");
$password = file_get_contents("../db_connection_info/dbPassword.dbase");
$database = file_get_contents("../db_connection_info/dbDatabase.dbase");

$connection = new mysqli($servername, $username, $password, $database);

if ($connection->connect_error) {
    die("Connection failed: " . $connection->connect_error);
}