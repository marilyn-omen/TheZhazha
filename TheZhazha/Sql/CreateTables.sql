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

CREATE TABLE IF NOT EXISTS
    stats
    (
        id INTEGER PRIMARY KEY,
        user VARCHAR NOT NULL,
        chat VARCHAR NOT NULL,
        started VARCHAR NOT NULL,
        updated VARCHAR NOT NULL,
        messages INTEGER NOT NULL,
        words INTEGER NOT NULL,
        symbols INTEGER NOT NULL,
        commands INTEGER NOT NULL
    );

CREATE TABLE IF NOT EXISTS
    settings
    (
        id INTEGER PRIMARY KEY,
        chat VARCHAR NOT NULL,
        reply INTEGER NOT NULL,
        vbros INTEGER NOT NULL,
        babka INTEGER NOT NULL
    );