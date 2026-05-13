-- Drop only the old constraint that's blocking submissions
ALTER TABLE FormSubmission DROP INDEX unique_submission;
