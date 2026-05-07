-- Fix FormSubmission with proper FK handling
USE EvaluaTeach;

-- Step 1: Drop foreign keys from SurveyResponse
SET FOREIGN_KEY_CHECKS = 0;

-- Step 2: Drop and recreate FormSubmission
DROP TABLE IF EXISTS FormSubmission;

CREATE TABLE FormSubmission (
    SubmissionID INT AUTO_INCREMENT PRIMARY KEY,
    StudentIDNumber VARCHAR(50),
    EvaluationID INT,
    SubmittedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StudentIDNumber) REFERENCES Student(IDNumber),
    FOREIGN KEY (EvaluationID) REFERENCES EvaluationForm(EvaluationID),
    UNIQUE KEY unique_submission (StudentIDNumber, EvaluationID)
);

-- Step 3: Re-enable FK checks and fix SurveyResponse
SET FOREIGN_KEY_CHECKS = 1;

-- Check SurveyResponse structure
DESCRIBE SurveyResponse;

-- If SurveyResponse has wrong columns, we may need to fix it too
