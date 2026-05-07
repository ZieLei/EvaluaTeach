-- Quick fix for StudentIDNumber column error
USE EvaluaTeach;

-- Check if Student table has IDNumber
SHOW COLUMNS FROM Student;

-- Create FormSubmission if missing
CREATE TABLE IF NOT EXISTS FormSubmission (
    SubmissionID INT AUTO_INCREMENT PRIMARY KEY,
    StudentIDNumber VARCHAR(50),
    EvaluationID INT,
    SubmittedAt DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY (StudentIDNumber) REFERENCES Student(IDNumber),
    FOREIGN KEY (EvaluationID) REFERENCES EvaluationForm(EvaluationID),
    UNIQUE KEY unique_submission (StudentIDNumber, EvaluationID)
);

-- Also make sure Password column exists in Admin
ALTER TABLE Admin ADD COLUMN IF NOT EXISTS Password VARCHAR(255);
UPDATE Admin SET Password = '2c26b46b68ffc68ff99b453c1d30413413422d706483bfa0f98a5e886266e7ae' 
WHERE Email = 'admin@evaluateach.edu' AND Password IS NULL;

-- Make sure default users exist
INSERT IGNORE INTO Student (IDNumber, FirstName, LastName, Email, Course, YearLevel, Password)
VALUES ('2024-000001', 'Juan', 'Dela Cruz', 'juan@student.edu', 'BSIT', 2, '2c26b46b68ffc68ff99b453c1d30413413422d706483bfa0f98a5e886266e7ae');
