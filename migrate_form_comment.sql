-- Migration: Extend the existing `comment` table for moderation system
-- Run this against your EvaluaTeach database before launching the updated build.

ALTER TABLE comment
    ADD COLUMN IF NOT EXISTS SubmissionID  INT          NULL,
    ADD COLUMN IF NOT EXISTS FormTitle     VARCHAR(300) NULL,
    ADD COLUMN IF NOT EXISTS SystemLevel   VARCHAR(10)  NOT NULL DEFAULT 'Normal',
    ADD COLUMN IF NOT EXISTS AdminLevel    VARCHAR(10)  NULL,
    ADD COLUMN IF NOT EXISTS ReviewedAt    DATETIME     NULL,
    ADD COLUMN IF NOT EXISTS ReviewedBy    VARCHAR(150) NULL;
