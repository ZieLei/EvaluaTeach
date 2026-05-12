-- Migration: Fix report table constraints and ensure schema matches application expectations
-- Run this against your EvaluaTeach database before launching the updated build.

-- Drop any CHECK constraint on ReportData (MariaDB/MySQL syntax)
ALTER TABLE report
    MODIFY COLUMN ReportData LONGTEXT NULL,
    MODIFY COLUMN AverageScore DECIMAL(5,2) NULL DEFAULT 0,
    MODIFY COLUMN ResponseCount INT NULL DEFAULT 0,
    MODIFY COLUMN EvaluationID INT NULL,
    MODIFY COLUMN TeacherID INT NULL,
    MODIFY COLUMN SubmissionDate DATETIME NULL DEFAULT CURRENT_TIMESTAMP;
