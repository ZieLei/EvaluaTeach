-- Clean all database tables except Admin login
-- Run this against your EvaluaTeach database

-- Disable foreign key checks to allow deletion in any order
SET FOREIGN_KEY_CHECKS = 0;

-- Delete data from all tables (order doesn't matter with FK checks disabled)
DELETE FROM comment;
DELETE FROM report;
DELETE FROM SurveyResponse;
DELETE FROM FormSubmission;
DELETE FROM QuestionOption;
DELETE FROM SurveyQuestion;
DELETE FROM EvaluationForm;
DELETE FROM Teacher;
DELETE FROM student;

-- Re-enable foreign key checks
SET FOREIGN_KEY_CHECKS = 1;

-- Reset auto-increment counters (optional - if you want IDs to start from 1 again)
ALTER TABLE comment AUTO_INCREMENT = 1;
ALTER TABLE report AUTO_INCREMENT = 1;
ALTER TABLE SurveyResponse AUTO_INCREMENT = 1;
ALTER TABLE FormSubmission AUTO_INCREMENT = 1;
ALTER TABLE QuestionOption AUTO_INCREMENT = 1;
ALTER TABLE SurveyQuestion AUTO_INCREMENT = 1;
ALTER TABLE EvaluationForm AUTO_INCREMENT = 1;
ALTER TABLE Teacher AUTO_INCREMENT = 1;
ALTER TABLE student AUTO_INCREMENT = 1;

-- Verify Admin table is untouched (should return your admin record)
SELECT * FROM Admin;
