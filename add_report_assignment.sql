-- Add AssignmentID to report table to track which subject the report is for
ALTER TABLE report
ADD COLUMN AssignmentID INT NULL AFTER EvaluationID,
ADD CONSTRAINT fk_report_assignment
FOREIGN KEY (AssignmentID) REFERENCES teacher_assignment(AssignmentID)
ON DELETE SET NULL;
