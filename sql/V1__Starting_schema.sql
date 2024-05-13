create schema if not exists climate_ctrl;

create or replace function climate_ctrl.pick_name() returns text
    language plpgsql
as
$$
DECLARE
    adjectives TEXT[] := ARRAY[
        'agreeable', 'alert', 'amused', 'brave', 'bright', 'charming', 'cheerful', 'comfortable',
        'cooperative', 'courageous', 'delightful', 'determined', 'eager', 'elated', 'enchanting',
        'encouraging', 'energetic', 'enthusiastic', 'excited', 'exuberant', 'faithful', 'fantastic',
        'friendly', 'funny', 'gentle', 'glorious', 'good', 'happy', 'healthy', 'helpful', 'hilarious',
        'innocent', 'jolly', 'kind', 'lively', 'lovely', 'lucky', 'obedient', 'perfect', 'proud',
        'relaxed', 'relieved', 'silly', 'smiling', 'splendid', 'successful', 'thoughtful', 'victorious',
        'vivacious', 'well', 'witty', 'wonderful', 'adorable', 'beautiful', 'bright', 'clean', 'clear',
        'colourful', 'curious', 'cute', 'elegant', 'fancy', 'gleaming', 'graceful', 'light', 'poised',
        'sparkling', 'spotless'
        ];
    animals TEXT[] := ARRAY[
        'alligator', 'crocodile', 'alpaca', 'ant', 'antelope', 'ape', 'armadillo', 'donkey', 'baboon',
        'badger', 'bat', 'bear', 'beaver', 'bee', 'beetle', 'buffalo', 'butterfly', 'camel', 'cat', 'cattle',
        'cheetah', 'chimpanzee', 'cicada', 'clam', 'coyote', 'crab', 'cricket', 'crow', 'raven', 'deer',
        'dinosaur', 'dog', 'dolphin', 'duck', 'eel', 'elephant', 'elk', 'ferret', 'fish', 'fly', 'fox', 'frog',
        'gerbil', 'giraffe', 'gnat', 'gnu', 'goat', 'goldfish', 'gorilla', 'grasshopper', 'hamster', 'hare',
        'hedgehog', 'herring', 'hippo', 'hornet', 'horse', 'hound', 'hyena', 'insect', 'jackal', 'jellyfish',
        'kangaroo', 'leopard', 'lion', 'lizard', 'llama', 'louse', 'mammoth', 'manatee', 'mink', 'mole',
        'monkey', 'moose', 'mosquito', 'mouse', 'mule', 'otter', 'ox', 'oyster', 'panda', 'pig', 'platypus',
        'porcupine', 'rabbit', 'raccoon', 'reindeer', 'rhino', 'salmon', 'sardine', 'scorpion', 'seal', 'shark',
        'sheep', 'snail', 'snake', 'spider', 'squirrel', 'termite', 'tiger', 'trout', 'turtle', 'walrus', 'yak'
        ];
    adj_index INT;
    animal_index INT;
    d_name TEXT;
    name_ok BOOLEAN;
BEGIN
    LOOP
        adj_index := floor(random() * array_length(adjectives, 1) + 1);
        animal_index := floor(random() * array_length(animals, 1) + 1);
        d_name := adjectives[adj_index] || '-' || animals[animal_index];

        -- Check if the name is okay (You need to define the conditions for a name to be okay)
        -- For example, if you have a table storing existing names, you can check if the generated name already exists in the table.

        -- Assuming a table named existing_names with a column name
        name_ok := NOT EXISTS (SELECT 1 FROM climate_ctrl.devices WHERE device_name = d_name);

        EXIT WHEN name_ok;
    END LOOP;

    RETURN d_name;
END;
$$;

create table if not exists climate_ctrl.device_status
(
    id    serial
        constraint device_status_pk
            primary key,
    value varchar(16) not null
);


create table if not exists climate_ctrl.devices
(
    id          serial
        constraint devices_pk
            primary key,
    mac         varchar(32)                                  not null
        constraint mac_uq
            unique,
    device_name varchar(64) default climate_ctrl.pick_name() not null
        constraint name_uq
            unique,
    status_id   integer     default 1
        constraint devices_device_status_id_fk
            references climate_ctrl.device_status
            on update cascade
);


create table if not exists climate_ctrl.bme_data
(
    id            serial
        constraint bme_data_pk
            primary key,
    device_mac    varchar(32)                                                       not null
        constraint bme_data_devices_mac_fk
            references climate_ctrl.devices (mac)
            on update cascade on delete cascade,
    temperature_c real,
    pressure      real,
    humidity      real,
    utc_time      timestamp with time zone default (now() AT TIME ZONE 'utc'::text) not null
);


create table if not exists climate_ctrl.device_config
(
    mac                 varchar(32)           not null
        constraint device_config_pk
            primary key
        constraint device_config_devices_mac_fk
            references climate_ctrl.devices (mac)
            on update cascade on delete cascade,
    last_motor_position integer,
    max_motor_position  integer,
    motor_reversed      boolean default false not null
);




