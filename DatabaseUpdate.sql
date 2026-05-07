-- Database Migration Script for EvaluaTeach
-- Run this in phpMyAdmin or MySQL CLI to update existing schema

USE EvaluaTeach;

-- ============================================
-- 1. Update EvaluationForm table
-- ============================================

-- Add missing columns
ALTER TABLE EvaluationForm 
    ADD COLUMN IF NOT EXISTS Description TEXT AFTER Title,
    ADD COLUMN IF NOT EXISTS TargetCourse VARCHAR(50) DEFAULT 'All' AFTER TeacherID,
    ADD COLUMN IF NOT EXISTS DueDate DATE AFTER TargetCourse,
    ADD COLUMN IF NOT EXISTS IsActive BOOLEAN DEFAULT TRUE AFTER DueDate,
    MODIFY COLUMN DateCreated DATETIME DEFAULT CURRENT_TIMESTAMP;

-- Drop redundant TargetDepartment if exists
ALTER TABLE EvaluationForm DROP COLUMN IF EXISTS TargetDepartment;

-- ============================================
-- 2. Update Admin table (add Password column)
-- ============================================
ALTER TABLE Admin 
    ADD COLUMN IF NOT EXISTS Password VARCHAR(255) NOT NULL AFTER AccessLevel,
    ADD COLUMN IF NOT EXISTS CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP AFTER Password;

-- ============================================
-- 3. Update Student table
-- ============================================
ALTER TABLE Student 
    ADD COLUMN IF NOT EXISTS CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP AFTER Password;

-- ============================================
-- 4. Update Teacher table
-- ============================================
ALTER TABLE Teacher 
    ADD COLUMN IF NOT EXISTS CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP AFTER Department;

-- ============================================
-- 5. Update SurveyQuestion table
-- ============================================

-- Add new columns for question configuration
ALTER TABLE SurveyQuestion 
    ADD COLUMN IF NOT EXISTS OrderIndex INT DEFAULT 0 AFTER QuestionType,
    ADD COLUMN IF NOT EXISTS IsRequired BOOLEAN DEFAULT TRUE AFTER OrderIndex,
    ADD COLUMN IF NOT EXISTS MinRating INT AFTER IsRequired,
    ADD COLUMN IF NOT EXISTS MaxRating INT AFTER MinRating;

-- Update QuestionType enum to include multiplechoice
-- Note: This requires recreating the column if enum values changed
-- For MySQL 8.0+ you can use:
-- ALTER TABLE SurveyQuestion 
--     MODIFY COLUMN QuestionType ENUM('rating', 'text', 'yesno', 'multiplechoice') DEFAULT 'rating';

-- ============================================
-- 6. Create QuestionOption table (for Multiple Choice)
-- ============================================
CREATE TABLE IF NOT EXISTS QuestionOption (
    OptionID INT AUTO_INCREMENT PRIMARY KEY,
    QuestionID INT,
    OptionText VARCHAR(255) NOT NULL,
    OrderIndex INT DEFAULT 0,
    FOREIGN KEY (QuestionID) REFERENCES SurveyQuestion(QuestionID) ON DELETE CASCADE
);

-- ============================================
-- 7. Create FormSubmission table (tracks form completions)
-- ============================================
CREATE TABLE IF NOT EXISTS FormSubmission (
    SubmissionID INT AUTO_INCREMENT PRIMARY KEY,
    StudentIDNumber VARCHAR(50),
    EvaluationID INT,
    SubmittedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StudentIDNumber) REFERENCES Student(IDNumber),
    FOREIGN KEY (EvaluationID) REFERENCES EvaluationForm(EvaluationID),
    UNIQUE KEY unique_submission (StudentIDNumber, EvaluationID)
);

-- ============================================
-- 8. Update SurveyResponse table (link to Submission instead of Student)
-- ============================================

-- Backup existing data if needed
-- Then recreate SurveyResponse with proper structure

-- Drop old foreign keys if exist
ALTER TABLE SurveyResponse 
    DROP FOREIGN KEY IF EXISTS surveyresponse_ibfk_1,
    DROP FOREIGN KEY IF EXISTS surveyresponse_ibfk_2,
    DROP COLUMN IF EXISTS StudentID,
    DROP COLUMN IF EXISTS DateSubmitted;

-- Modify columns
ALTER TABLE SurveyResponse 
    ADD COLUMN IF NOT EXISTS SubmissionID INT AFTER ResponseID,
    MODIFY COLUMN QuestionID INT,
    ADD FOREIGN KEY IF NOT EXISTS (SubmissionID) REFERENCES FormSubmission(SubmissionID) ON DELETE CASCADE,
    ADD FOREIGN KEY IF NOT EXISTS (QuestionID) REFERENCES SurveyQuestion(QuestionID);

-- ============================================
-- 9. Update Report table
-- ============================================
ALTER TABLE Report 
    MODIFY COLUMN SubmissionDate DATETIME DEFAULT CURRENT_TIMESTAMP,
    ADD COLUMN IF NOT EXISTS ResponseCount INT AFTER AverageScore,
    ADD COLUMN IF NOT EXISTS ReportData JSON AFTER ResponseCount,
    MODIFY COLUMN EvaluationID INT;

-- ============================================
-- 10. Update Comment table (remove ENUM for compatibility)
-- ============================================
ALTER TABLE Comment 
    MODIFY COLUMN Status VARCHAR(20) DEFAULT 'pending';

-- ============================================
-- 11. Update Subject table
-- ============================================
ALTER TABLE Subject 
    ADD FOREIGN KEY IF NOT EXISTS (TeacherID) REFERENCES Teacher(TeacherID) ON DELETE SET NULL;

-- ============================================
-- 12. Insert default admin and student for testing
-- ============================================

-- Default Admin (password: admin123, hashed)
INSERT IGNORE INTO Admin (FirstName, LastName, Email, AccessLevel, Password)
VALUES ('System', 'Admin', 'admin@evaluateach.edu', 'SuperAdmin', '2c26b46b68ffc68ff99b453c1d30413413422d706483bfa0f98a5e886266e7ae');

-- Default Student (password: student123, hashed)
INSERT IGNORE INTO Student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Password)
VALUES ('2024-000001', 'Juan', 'Dela Cruz', 'juan@student.edu', 'BSIT', 2, '2c26b46b68ffc68ff99b453c1d30413413422d706483bfa0f98a5e886266e7ae');

INSERT IGNORE INTO Student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Password)
VALUES ('2024-000002', 'Maria', 'Santos', 'maria@student.edu', 'BSCS', 3, '2c26b46b68ffc68ff99b453c1d30413413422d706483bfa0f98a5e886266e7ae');

-- ============================================
-- DONE! Test with:
-- SELECT * FROM Admin;
-- SELECT * FROM Student;
-- ============================================
