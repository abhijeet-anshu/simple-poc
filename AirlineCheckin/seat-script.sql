use mysql;

show tables;

select * from seats;

truncate table seats;

select * from seats;

START TRANSACTION;
select seat_id, seat_name
FROM seats
    WHERE user_id IS not NULL
    ORDER BY seat_id
    LIMIT 1
    FOR UPDATE SKIP LOCKED
;
ROLLBACK;

UPDATE seats
SET user_id = 50
WHERE seat_id = (
    SELECT TOP 1 seat_id
    FROM seats
    WHERE user_id IS NULL
    ORDER BY seat_id
    FOR UPDATE
);

COMMIT;

Error Code: 1064. You have an error in your SQL syntax; 
check the manual that corresponds to your MySQL server version for the right syntax to use near 
'1 seat_id     FROM seats     WHERE user_id IS NULL     ORDER BY seat_id     FOR ' at line 4


select user_id from seats where seat_id='2C';

select count(*) from seats;

drop table seats;


CREATE TABLE seats (
    seat_id INT NOT NULL AUTO_INCREMENT,
    seat_name VARCHAR(5),
    user_id INT,
    PRIMARY KEY (seat_id),
    KEY (seat_name)
);


CREATE TABLE seats (
    seat_id VARCHAR(5) NOT NULL PRIMARY KEY,
    user_id INT
);
