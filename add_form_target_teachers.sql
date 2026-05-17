-- Add TargetTeacherIds column to EvaluationForm table for multiple teacher targeting
ALTER TABLE EvaluationForm
ADD COLUMN TargetTeacherIds VARCHAR(500) NULL AFTER TargetTeacherId,
ADD COLUMN TargetMode ENUM('All', 'Course', 'Teachers') DEFAULT 'All' AFTER TargetTeacherIds;
