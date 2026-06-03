-- +goose Up
CREATE TABLE IF NOT EXISTS generation_history (
    id BIGSERIAL PRIMARY KEY,
    user_id TEXT NOT NULL,
    prompt TEXT NOT NULL,
    plan_name VARCHAR(255) NOT NULL,
    plan_json TEXT NOT NULL,
    created_at TIMESTAMP WITH TIME ZONE NOT NULL DEFAULT NOW()
);
CREATE INDEX IF NOT EXISTS idx_generation_history_user_id ON generation_history (user_id);
-- +goose Down
DROP TABLE IF EXISTS generation_history;
