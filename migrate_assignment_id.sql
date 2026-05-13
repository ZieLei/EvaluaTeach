-- Migration: Add AssignmentID to FormSubmission for per-subject evaluation tracking
-- This allows students to evaluate the same teacher once per subject

ALTER TABLE FormSubmission
ADD COLUMN AssignmentID INT NULL AFTER TeacherID,
ADD CONSTRAINT fk_formsubmission_assignment
    FOREIGN KEY (AssignmentID) REFERENCES teacher_assignment(AssignmentID)
    ON DELETE SET NULL ON UPDATE CASCADE;

-- Create index for faster lookups
CREATE INDEX idx_formsubmission_assignment ON FormSubmission(AssignmentID);
