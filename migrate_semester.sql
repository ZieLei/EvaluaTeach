ALTER TABLE EvaluationForm
  ADD COLUMN Semester  ENUM('1st','2nd','Summer') NULL AFTER IsActive,
  ADD COLUMN SchoolYear VARCHAR(9) NULL AFTER Semester;
-- e.g. SchoolYear = '2024-2025'
-- Existing rows will have NULL for both columns (treated as "no semester set").
