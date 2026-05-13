-- Migration: Update unique constraint to allow per-subject evaluations
-- Drop old constraint and add new one including AssignmentID

-- First, drop the existing unique constraint
ALTER TABLE FormSubmission DROP INDEX unique_submission;

-- Add new unique constraint that includes AssignmentID
-- This allows: same student, same form, same teacher, but DIFFERENT subjects
ALTER TABLE FormSubmission
ADD CONSTRAINT unique_submission_per_subject 
UNIQUE (StudentIDNumber, EvaluationID, TeacherID, AssignmentID);
