-- Fix FormSubmission table to match code
USE EvaluaTeach;

-- Drop and recreate with correct column name
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

-- Also fix SurveyResponse table
ALTER TABLE SurveyResponse 
    ADD COLUMN IF NOT EXISTS SubmissionID INT AFTER ResponseID;

-- If SubmissionID exists but has wrong FK, fix it:
-- ALTER TABLE SurveyResponse DROP FOREIGN KEY IF EXISTS surveyresponse_ibfk_3;
-- ALTER TABLE SurveyResponse ADD FOREIGN KEY (SubmissionID) REFERENCES FormSubmission(SubmissionID) ON DELETE CASCADE;

SHOW TABLES;
DESCRIBE FormSubmission;
