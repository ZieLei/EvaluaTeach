-- Fix unique constraint for per-subject evaluation
-- Handle both KEY and INDEX naming

-- Drop existing constraints (try both KEY and INDEX syntax)
ALTER TABLE FormSubmission DROP INDEX IF EXISTS unique_submission;
ALTER TABLE FormSubmission DROP INDEX IF EXISTS unique_submission_per_subject;

-- Handle existing data: set AssignmentID=1 for old submissions that have NULL
-- (temporary assignment ID to satisfy constraint)
-- UPDATE FormSubmission SET AssignmentID = 1 WHERE AssignmentID IS NULL;

-- Add new unique constraint that includes AssignmentID
-- This allows: same student, same form, same teacher, different subjects
ALTER TABLE FormSubmission
ADD CONSTRAINT unique_submission_per_subject 
UNIQUE (StudentIDNumber, EvaluationID, TeacherID, AssignmentID);
