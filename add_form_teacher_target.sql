-- Add TargetTeacherId column to EvaluationForm table
ALTER TABLE EvaluationForm
ADD COLUMN TargetTeacherId INT NULL AFTER TargetCourse,
ADD CONSTRAINT fk_form_teacher
FOREIGN KEY (TargetTeacherId) REFERENCES Teacher(TeacherID)
ON DELETE SET NULL;
