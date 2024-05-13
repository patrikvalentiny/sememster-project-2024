INSERT INTO climate_ctrl.device_status (id, value) VALUES (1, 'online') ON CONFLICT DO NOTHING;
INSERT INTO climate_ctrl.device_status (id, value) VALUES (2, 'offline') ON CONFLICT DO NOTHING;
INSERT INTO climate_ctrl.device_status (id, value) VALUES (3, 'suspended') ON CONFLICT DO NOTHING;
