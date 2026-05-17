-- Migration: Add columns to support text question answers in comment review
-- This allows flagged text answers to be reviewed alongside additional comments

ALTER TABLE `comment`
ADD COLUMN `QuestionId` INT(11) DEFAULT NULL AFTER `ReviewedBy`,
ADD COLUMN `QuestionText` VARCHAR(500) DEFAULT NULL AFTER `QuestionId`;

-- Update existing comments to have QuestionId = NULL (they are additional comments, not text answers)
-- No data migration needed as NULL is the default
