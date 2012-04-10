CREATE TABLE IF NOT EXISTS
    quots
    (
        chat VARCHAR NOT NULL,
        quot TEXT NOT NULL,
        user VARCHAR NOT NULL,
        date VARCHAR NOT NULL
    );

CREATE TABLE IF NOT EXISTS
    banned
    (
        user VARCHAR NOT NULL,
        date VARCHAR NOT NULL,
        reason VARCHAR NOT NULL,
        chat VARCHAR NOT NULL
    );

CREATE TABLE IF NOT EXISTS
    warnings
    (
        user VARCHAR NOT NULL,
        chat VARCHAR NOT NULL,
        date VARCHAR NOT NULL,
        reason VARCHAR NOT NULL
    );

CREATE TABLE IF NOT EXISTS
    admins
    (
        user VARCHAR NOT NULL,
        chat VARCHAR NOT NULL
    );