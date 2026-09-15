-- Initialize database and create temperatures table for TelDemo.WebApi
CREATE DATABASE IF NOT EXISTS `teldemo`;
USE `teldemo`;

CREATE TABLE IF NOT EXISTS `temperatures` (
    `id` INT AUTO_INCREMENT PRIMARY KEY,
    `date` DATE NOT NULL UNIQUE,
    `temperature_c` INT NOT NULL,
    `summary` VARCHAR(100) NOT NULL,
    `created_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

-- Pre-populate with temperature forecast data relative to current date
INSERT INTO `temperatures` (`date`, `temperature_c`, `summary`)
VALUES
    (DATE_SUB(CURDATE(), INTERVAL 2 DAY), 12, 'Bracing'),
    (DATE_SUB(CURDATE(), INTERVAL 1 DAY), 15, 'Chilly'),
    (CURDATE(), 19, 'Mild'),
    (DATE_ADD(CURDATE(), INTERVAL 1 DAY), 22, 'Warm'),
    (DATE_ADD(CURDATE(), INTERVAL 2 DAY), 25, 'Balmy'),
    (DATE_ADD(CURDATE(), INTERVAL 3 DAY), 31, 'Hot'),
    (DATE_ADD(CURDATE(), INTERVAL 4 DAY), 34, 'Sweltering'),
    (DATE_ADD(CURDATE(), INTERVAL 5 DAY), 37, 'Scorching'),
    (DATE_ADD(CURDATE(), INTERVAL 6 DAY), 18, 'Cool'),
    (DATE_ADD(CURDATE(), INTERVAL 7 DAY), 14, 'Chilly'),
    (DATE_ADD(CURDATE(), INTERVAL 8 DAY), -3, 'Freezing'),
    (DATE_ADD(CURDATE(), INTERVAL 9 DAY), 8, 'Bracing'),
    (DATE_ADD(CURDATE(), INTERVAL 10 DAY), 16, 'Cool'),
    (DATE_ADD(CURDATE(), INTERVAL 11 DAY), 20, 'Mild'),
    (DATE_ADD(CURDATE(), INTERVAL 12 DAY), 23, 'Warm'),
    (DATE_ADD(CURDATE(), INTERVAL 13 DAY), 27, 'Balmy'),
    (DATE_ADD(CURDATE(), INTERVAL 14 DAY), 29, 'Hot')
ON DUPLICATE KEY UPDATE
    `temperature_c` = VALUES(`temperature_c`),
    `summary` = VALUES(`summary`);
