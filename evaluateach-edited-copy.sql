-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: May 17, 2026 at 03:51 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `evaluateach`
--

-- --------------------------------------------------------

--
-- Table structure for table `admin`
--

CREATE TABLE `admin` (
  `AdminID` int(11) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `AccessLevel` varchar(50) DEFAULT NULL,
  `Password` varchar(255) NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `admin`
--

INSERT INTO `admin` (`AdminID`, `FirstName`, `LastName`, `Email`, `AccessLevel`, `Password`, `CreatedAt`) VALUES
(1, 'System', 'Admin', 'admin@evaluateach.edu', 'SuperAdmin', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', '2026-05-07 21:23:28');

-- --------------------------------------------------------

--
-- Table structure for table `comment`
--

CREATE TABLE `comment` (
  `CommentID` int(11) NOT NULL,
  `StudentID` int(11) DEFAULT NULL,
  `TeacherID` int(11) DEFAULT NULL,
  `Content` text DEFAULT NULL,
  `DateSubmitted` datetime DEFAULT current_timestamp(),
  `Status` varchar(20) DEFAULT 'pending',
  `SubmissionID` int(11) DEFAULT NULL,
  `FormTitle` varchar(300) DEFAULT NULL,
  `SystemLevel` varchar(10) NOT NULL DEFAULT 'Normal',
  `AdminLevel` varchar(10) DEFAULT NULL,
  `ReviewedAt` datetime DEFAULT NULL,
  `ReviewedBy` varchar(150) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `comment`
--

INSERT INTO `comment` (`CommentID`, `StudentID`, `TeacherID`, `Content`, `DateSubmitted`, `Status`, `SubmissionID`, `FormTitle`, `SystemLevel`, `AdminLevel`, `ReviewedAt`, `ReviewedBy`) VALUES
(8, 995, 4, 'WHAHAHAHAH', '2026-05-15 13:40:08', 'Approved', 23, 'Test', 'Normal', NULL, NULL, NULL);

-- --------------------------------------------------------

--
-- Table structure for table `evaluationform`
--

CREATE TABLE `evaluationform` (
  `EvaluationID` int(11) NOT NULL,
  `Title` varchar(100) NOT NULL,
  `Description` text DEFAULT NULL,
  `CreatedBy` int(11) DEFAULT NULL,
  `TeacherID` int(11) DEFAULT NULL,
  `TargetCourse` varchar(50) DEFAULT 'All',
  `TargetTeacherId` int(11) DEFAULT NULL,
  `DueDate` date DEFAULT NULL,
  `IsActive` tinyint(1) DEFAULT 1,
  `Semester` enum('1st','2nd','Summer') DEFAULT NULL,
  `SchoolYear` varchar(9) DEFAULT NULL,
  `DateCreated` datetime DEFAULT current_timestamp(),
  `TargetTeacherIds` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `evaluationform`
--

INSERT INTO `evaluationform` (`EvaluationID`, `Title`, `Description`, `CreatedBy`, `TeacherID`, `TargetCourse`, `TargetTeacherId`, `DueDate`, `IsActive`, `Semester`, `SchoolYear`, `DateCreated`, `TargetTeacherIds`) VALUES
(2, 'General Teacher Evaluation Form', 'a evaluation form to wrap up the school semester and address issues.', 1, NULL, 'All', NULL, '2026-05-20', 1, '2nd', '2025-2026', '2026-05-12 13:28:36', NULL),
(8, 'Test', 'TESRTD', 1, NULL, 'All', NULL, '2026-05-15', 0, '2nd', '2025-2026', '2026-05-13 13:40:50', NULL),
(9, 'New Form', 'form for testing', 1, NULL, 'All', NULL, NULL, 1, '2nd', '2022-2023', '2026-05-17 21:12:56', NULL);

-- --------------------------------------------------------

--
-- Table structure for table `formsubmission`
--

CREATE TABLE `formsubmission` (
  `SubmissionID` int(11) NOT NULL,
  `StudentIDNumber` varchar(50) DEFAULT NULL,
  `EvaluationID` int(11) DEFAULT NULL,
  `SubmittedAt` datetime DEFAULT current_timestamp(),
  `TeacherID` int(11) DEFAULT NULL,
  `AssignmentID` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `formsubmission`
--

INSERT INTO `formsubmission` (`SubmissionID`, `StudentIDNumber`, `EvaluationID`, `SubmittedAt`, `TeacherID`, `AssignmentID`) VALUES
(26, '2024004777', 2, '2026-05-17 21:14:09', 4, 6);

-- --------------------------------------------------------

--
-- Table structure for table `questionoption`
--

CREATE TABLE `questionoption` (
  `OptionID` int(11) NOT NULL,
  `QuestionID` int(11) DEFAULT NULL,
  `OptionText` varchar(255) NOT NULL,
  `OrderIndex` int(11) DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `questionoption`
--

INSERT INTO `questionoption` (`OptionID`, `QuestionID`, `OptionText`, `OrderIndex`) VALUES
(94, 235, 'Very often', 0),
(95, 235, 'Often', 1),
(96, 235, 'Rarely', 2),
(106, 275, 'Very Clear', 0),
(107, 275, 'Clear', 1),
(108, 275, 'Somewhat Clear', 2),
(109, 275, 'Unclear', 3),
(110, 276, 'Always', 0),
(111, 276, 'Often', 1),
(112, 276, 'Sometimes', 2),
(113, 276, 'Rarely', 3),
(114, 276, 'Never', 4);

-- --------------------------------------------------------

--
-- Table structure for table `report`
--

CREATE TABLE `report` (
  `ReportID` int(11) NOT NULL,
  `TeacherID` int(11) DEFAULT NULL,
  `EvaluationID` int(11) DEFAULT NULL,
  `AssignmentID` int(11) DEFAULT NULL,
  `SubmissionDate` datetime DEFAULT current_timestamp(),
  `AverageScore` decimal(5,2) DEFAULT 0.00,
  `ResponseCount` int(11) DEFAULT 0,
  `ReportData` longtext DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `report`
--

INSERT INTO `report` (`ReportID`, `TeacherID`, `EvaluationID`, `AssignmentID`, `SubmissionDate`, `AverageScore`, `ResponseCount`, `ReportData`) VALUES
(18, 4, 2, 7, '2026-05-13 09:39:20', 3.80, 1, 'SubmissionID:19\r\nTeacher: Test Test\r\nDepartment: All\r\n------------------------------\r\n1. Demonstrates knowledge of subject matter\r\n   Answer: 5\r\n2. Explains concepts clearly\r\n   Answer: 4\r\n3. Encourages student participation\r\n   Answer: 3\r\n4. Manages Classroom effectively\r\n   Answer: 3\r\n5. Uses instructural time efficiently\r\n   Answer: 4\r\n6. Was the lesson well organized?\r\n   Answer: No\r\n7. Were learning objectives clearly stated?\r\n   Answer: Yes\r\n8. Did the teacher create a respectful environment?\r\n   Answer: No\r\n9. Were students engaged during the lesson?\r\n   Answer: Yes\r\n10. Did the teacher answer questions effectively?\r\n   Answer: No\r\n11. How would you describe the teacher\'s communication style?\r\n   Answer: Very Clear\r\n12. How often does the teacher encourage participation?\r\n   Answer: Often\r\n13. Areas for improvement\r\n   Answer: 123'),
(19, 4, 2, 6, '2026-05-13 09:39:23', 3.60, 1, 'SubmissionID:17\r\nTeacher: Test Test\r\nDepartment: All\r\n------------------------------\r\n1. Demonstrates knowledge of subject matter\r\n   Answer: 4\r\n2. Explains concepts clearly\r\n   Answer: 5\r\n3. Encourages student participation\r\n   Answer: 4\r\n4. Manages Classroom effectively\r\n   Answer: 3\r\n5. Uses instructural time efficiently\r\n   Answer: 2\r\n6. Was the lesson well organized?\r\n   Answer: No\r\n7. Were learning objectives clearly stated?\r\n   Answer: Yes\r\n8. Did the teacher create a respectful environment?\r\n   Answer: Yes\r\n9. Were students engaged during the lesson?\r\n   Answer: No\r\n10. Did the teacher answer questions effectively?\r\n   Answer: Yes\r\n11. How would you describe the teacher\'s communication style?\r\n   Answer: Very Clear\r\n12. How often does the teacher encourage participation?\r\n   Answer: Often\r\n13. Areas for improvement\r\n   Answer: asd'),
(22, 4, 2, 6, '2026-05-15 13:37:41', 3.20, 1, 'SubmissionID:22\r\nTeacher: Test Test\r\nDepartment: All\r\n------------------------------\r\n1. Demonstrates knowledge of subject matter\r\n   Answer: 3\r\n2. Explains concepts clearly\r\n   Answer: 5\r\n3. Encourages student participation\r\n   Answer: 4\r\n4. Manages Classroom effectively\r\n   Answer: 1\r\n5. Uses instructural time efficiently\r\n   Answer: 3\r\n6. Was the lesson well organized?\r\n   Answer: No\r\n7. Were learning objectives clearly stated?\r\n   Answer: Yes\r\n8. Did the teacher create a respectful environment?\r\n   Answer: Yes\r\n9. Were students engaged during the lesson?\r\n   Answer: Yes\r\n10. Did the teacher answer questions effectively?\r\n   Answer: Yes\r\n11. How would you describe the teacher\'s communication style?\r\n   Answer: Very Clear\r\n12. How often does the teacher encourage participation?\r\n   Answer: Always\r\n13. Areas for improvement\r\n   Answer: no comment'),
(24, 4, 2, 6, '2026-05-17 21:14:46', 5.00, 1, 'SubmissionID:26\r\nTeacher: Test Test\r\nDepartment: All\r\n------------------------------\r\n1. Demonstrates knowledge of subject matter\r\n   Answer: 5\r\n2. Explains concepts clearly\r\n   Answer: 5\r\n3. Encourages student participation\r\n   Answer: 5\r\n4. Manages Classroom effectively\r\n   Answer: 5\r\n5. Uses instructural time efficiently\r\n   Answer: 5\r\n6. Was the lesson well organized?\r\n   Answer: No\r\n7. Were learning objectives clearly stated?\r\n   Answer: No\r\n8. Did the teacher create a respectful environment?\r\n   Answer: No\r\n9. Were students engaged during the lesson?\r\n   Answer: No\r\n10. Did the teacher answer questions effectively?\r\n   Answer: No\r\n11. How would you describe the teacher\'s communication style?\r\n   Answer: Clear\r\n12. How often does the teacher encourage participation?\r\n   Answer: Always\r\n13. Areas for improvement\r\n   Answer: asd');

-- --------------------------------------------------------

--
-- Table structure for table `student`
--

CREATE TABLE `student` (
  `StudentID` int(11) NOT NULL,
  `IDNumber` varchar(50) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `Email` varchar(100) NOT NULL,
  `Course` varchar(50) DEFAULT NULL,
  `Section` varchar(20) DEFAULT NULL,
  `YearLevel` int(11) DEFAULT NULL,
  `Password` varchar(255) NOT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `Avatar` mediumblob DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `student`
--

INSERT INTO `student` (`StudentID`, `IDNumber`, `FirstName`, `LastName`, `Email`, `Course`, `Section`, `YearLevel`, `Password`, `CreatedAt`, `Avatar`) VALUES
(4, '2023000023', 'Angelika', 'Dela Cruz';

-- --------------------------------------------------------

--
-- Table structure for table `subject`
--

CREATE TABLE `subject` (
  `SubjectID` int(11) NOT NULL,
  `SubjectName` varchar(100) NOT NULL,
  `TeacherID` int(11) DEFAULT NULL,
  `Course` varchar(50) DEFAULT NULL,
  `Section` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

-- --------------------------------------------------------

--
-- Table structure for table `surveyquestion`
--

CREATE TABLE `surveyquestion` (
  `QuestionID` int(11) NOT NULL,
  `EvaluationID` int(11) DEFAULT NULL,
  `QuestionText` text NOT NULL,
  `Category` varchar(50) DEFAULT NULL,
  `QuestionType` enum('rating','text','yesno','multiplechoice') DEFAULT 'rating',
  `OrderIndex` int(11) DEFAULT 0,
  `IsRequired` tinyint(1) DEFAULT 1,
  `MinRating` int(11) DEFAULT NULL,
  `MaxRating` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `surveyquestion`
--

INSERT INTO `surveyquestion` (`QuestionID`, `EvaluationID`, `QuestionText`, `Category`, `QuestionType`, `OrderIndex`, `IsRequired`, `MinRating`, `MaxRating`) VALUES
(232, 9, 'Rate this teachers teaching', 'About teaching', 'rating', 0, 1, 1, 5),
(233, 9, 'Comments about the teaching method', 'About teaching', 'text', 1, 1, 1, 5),
(234, 9, 'Does the teacher do class activities?', 'About teaching', 'yesno', 2, 1, 1, 5),
(235, 9, 'How often does the teacher do class activities', 'About teaching', 'multiplechoice', 3, 1, 1, 5),
(236, 9, 'The teacher is always present', 'Attendance', 'rating', 4, 1, 1, 5),
(247, 8, 'Usdsdfs', 'Teaching', 'text', 0, 1, 1, 5),
(248, 8, 'Rate this aspect (1-5)wdfsd', 'Teaching', 'rating', 1, 1, 1, 5),
(249, 8, 'Rate this aspect (1-5)', 'Christine', 'rating', 2, 1, 1, 5),
(250, 8, 'Rate this aspect (1-5)', 'Tesst', 'rating', 3, 1, 1, 5),
(251, 8, 'Test Comment', 'Christine', 'text', 4, 1, 1, 5),
(265, 2, 'Demonstrates knowledge of subject matter', 'General', 'rating', 0, 1, 1, 5),
(266, 2, 'Explains concepts clearly', 'General', 'rating', 1, 1, 1, 5),
(267, 2, 'Encourages student participation', 'General', 'rating', 2, 1, 1, 5),
(268, 2, 'Manages Classroom effectively', 'General', 'rating', 3, 1, 1, 5),
(269, 2, 'Uses instructural time efficiently', 'General', 'rating', 4, 1, 1, 5),
(270, 2, 'Was the lesson well organized?', 'General', 'yesno', 5, 1, 1, 5),
(271, 2, 'Were learning objectives clearly stated?', 'General', 'yesno', 6, 1, 1, 5),
(272, 2, 'Did the teacher create a respectful environment?', 'General', 'yesno', 7, 1, 1, 5),
(273, 2, 'Were students engaged during the lesson?', 'General', 'yesno', 8, 1, 1, 5),
(274, 2, 'Did the teacher answer questions effectively?', 'General', 'yesno', 9, 1, 1, 5),
(275, 2, 'How would you describe the teacher\'s communication style?', 'General', 'multiplechoice', 10, 1, 1, 5),
(276, 2, 'How often does the teacher encourage participation?', 'General', 'multiplechoice', 11, 1, 1, 5),
(277, 2, 'Areas for improvement', 'General', 'text', 12, 1, 1, 5);

-- --------------------------------------------------------

--
-- Table structure for table `surveyresponse`
--

CREATE TABLE `surveyresponse` (
  `ResponseID` int(11) NOT NULL,
  `SubmissionID` int(11) DEFAULT NULL,
  `QuestionID` int(11) DEFAULT NULL,
  `Answer` text DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `surveyresponse`
--

INSERT INTO `surveyresponse` (`ResponseID`, `SubmissionID`, `QuestionID`, `Answer`) VALUES
(160, 26, 265, '5'),
(161, 26, 266, '5'),
(162, 26, 267, '5'),
(163, 26, 268, '5'),
(164, 26, 269, '5'),
(165, 26, 270, 'No'),
(166, 26, 271, 'No'),
(167, 26, 272, 'No'),
(168, 26, 273, 'No'),
(169, 26, 274, 'No'),
(170, 26, 275, 'Clear'),
(171, 26, 276, 'Always'),
(172, 26, 277, 'asd');

-- --------------------------------------------------------

--
-- Table structure for table `teacher`
--

CREATE TABLE `teacher` (
  `TeacherID` int(11) NOT NULL,
  `FirstName` varchar(50) NOT NULL,
  `LastName` varchar(50) NOT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Department` varchar(50) DEFAULT NULL,
  `Section` varchar(20) DEFAULT NULL,
  `Course` varchar(50) DEFAULT NULL,
  `YearLevel` int(11) DEFAULT NULL,
  `CreatedAt` datetime DEFAULT current_timestamp(),
  `Password` varchar(255) NOT NULL,
  `Subjects` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `teacher`
--

INSERT INTO `teacher` (`TeacherID`, `FirstName`, `LastName`, `Email`, `Department`, `Section`, `Course`, `YearLevel`, `CreatedAt`, `Password`, `Subjects`) VALUES
(4, 'Test', 'Test', 'test@gmail.com', 'BSIT', NULL, NULL, NULL, '2026-05-13 01:57:14', 'jZae727K08KaOmKSgOaGzww/XVqGr/PKEgIMkjrcbJI=', '');

-- --------------------------------------------------------

--
-- Table structure for table `teacher_assignment`
--

CREATE TABLE `teacher_assignment` (
  `AssignmentID` int(11) NOT NULL,
  `TeacherID` int(11) NOT NULL,
  `Section` varchar(20) DEFAULT NULL,
  `Course` varchar(50) DEFAULT NULL,
  `YearLevel` varchar(20) DEFAULT NULL,
  `Subjects` varchar(500) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `teacher_assignment`
--

INSERT INTO `teacher_assignment` (`AssignmentID`, `TeacherID`, `Section`, `Course`, `YearLevel`, `Subjects`) VALUES
(6, 4, 'C', 'BSIT', '2', 'Programming'),
(7, 4, 'B', 'BSIT', '1', 'Math'),
(8, 4, 'A', 'BSIT', '2', 'English');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `admin`
--
ALTER TABLE `admin`
  ADD PRIMARY KEY (`AdminID`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indexes for table `comment`
--
ALTER TABLE `comment`
  ADD PRIMARY KEY (`CommentID`),
  ADD KEY `StudentID` (`StudentID`),
  ADD KEY `TeacherID` (`TeacherID`);

--
-- Indexes for table `evaluationform`
--
ALTER TABLE `evaluationform`
  ADD PRIMARY KEY (`EvaluationID`),
  ADD KEY `CreatedBy` (`CreatedBy`),
  ADD KEY `TeacherID` (`TeacherID`);

--
-- Indexes for table `formsubmission`
--
ALTER TABLE `formsubmission`
  ADD PRIMARY KEY (`SubmissionID`),
  ADD UNIQUE KEY `unique_submission_per_subject` (`StudentIDNumber`,`EvaluationID`,`TeacherID`,`AssignmentID`),
  ADD KEY `formsubmission_ibfk_2` (`EvaluationID`),
  ADD KEY `formsubmission_ibfk_3` (`TeacherID`),
  ADD KEY `idx_formsubmission_assignment` (`AssignmentID`);

--
-- Indexes for table `questionoption`
--
ALTER TABLE `questionoption`
  ADD PRIMARY KEY (`OptionID`),
  ADD KEY `QuestionID` (`QuestionID`);

--
-- Indexes for table `report`
--
ALTER TABLE `report`
  ADD PRIMARY KEY (`ReportID`),
  ADD KEY `TeacherID` (`TeacherID`),
  ADD KEY `EvaluationID` (`EvaluationID`),
  ADD KEY `fk_report_assignment` (`AssignmentID`);

--
-- Indexes for table `student`
--
ALTER TABLE `student`
  ADD PRIMARY KEY (`StudentID`),
  ADD UNIQUE KEY `IDNumber` (`IDNumber`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indexes for table `studentslcc`
--
ALTER TABLE `studentslcc`
  ADD PRIMARY KEY (`id_number`);

--
-- Indexes for table `subject`
--
ALTER TABLE `subject`
  ADD PRIMARY KEY (`SubjectID`),
  ADD KEY `TeacherID` (`TeacherID`);

--
-- Indexes for table `surveyquestion`
--
ALTER TABLE `surveyquestion`
  ADD PRIMARY KEY (`QuestionID`),
  ADD KEY `EvaluationID` (`EvaluationID`);

--
-- Indexes for table `surveyresponse`
--
ALTER TABLE `surveyresponse`
  ADD PRIMARY KEY (`ResponseID`),
  ADD KEY `QuestionID` (`QuestionID`),
  ADD KEY `SubmissionID` (`SubmissionID`);

--
-- Indexes for table `teacher`
--
ALTER TABLE `teacher`
  ADD PRIMARY KEY (`TeacherID`),
  ADD UNIQUE KEY `Email` (`Email`);

--
-- Indexes for table `teacher_assignment`
--
ALTER TABLE `teacher_assignment`
  ADD PRIMARY KEY (`AssignmentID`),
  ADD KEY `TeacherID` (`TeacherID`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `admin`
--
ALTER TABLE `admin`
  MODIFY `AdminID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=3;

--
-- AUTO_INCREMENT for table `comment`
--
ALTER TABLE `comment`
  MODIFY `CommentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- AUTO_INCREMENT for table `evaluationform`
--
ALTER TABLE `evaluationform`
  MODIFY `EvaluationID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=10;

--
-- AUTO_INCREMENT for table `formsubmission`
--
ALTER TABLE `formsubmission`
  MODIFY `SubmissionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=27;

--
-- AUTO_INCREMENT for table `questionoption`
--
ALTER TABLE `questionoption`
  MODIFY `OptionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=115;

--
-- AUTO_INCREMENT for table `report`
--
ALTER TABLE `report`
  MODIFY `ReportID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=25;

--
-- AUTO_INCREMENT for table `student`
--
ALTER TABLE `student`
  MODIFY `StudentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=4672;

--
-- AUTO_INCREMENT for table `subject`
--
ALTER TABLE `subject`
  MODIFY `SubjectID` int(11) NOT NULL AUTO_INCREMENT;

--
-- AUTO_INCREMENT for table `surveyquestion`
--
ALTER TABLE `surveyquestion`
  MODIFY `QuestionID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=278;

--
-- AUTO_INCREMENT for table `surveyresponse`
--
ALTER TABLE `surveyresponse`
  MODIFY `ResponseID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=173;

--
-- AUTO_INCREMENT for table `teacher`
--
ALTER TABLE `teacher`
  MODIFY `TeacherID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=5;

--
-- AUTO_INCREMENT for table `teacher_assignment`
--
ALTER TABLE `teacher_assignment`
  MODIFY `AssignmentID` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=9;

--
-- Constraints for dumped tables
--

--
-- Constraints for table `comment`
--
ALTER TABLE `comment`
  ADD CONSTRAINT `comment_ibfk_1` FOREIGN KEY (`StudentID`) REFERENCES `student` (`StudentID`),
  ADD CONSTRAINT `comment_ibfk_2` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`);

--
-- Constraints for table `evaluationform`
--
ALTER TABLE `evaluationform`
  ADD CONSTRAINT `evaluationform_ibfk_1` FOREIGN KEY (`CreatedBy`) REFERENCES `admin` (`AdminID`),
  ADD CONSTRAINT `evaluationform_ibfk_2` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`);

--
-- Constraints for table `formsubmission`
--
ALTER TABLE `formsubmission`
  ADD CONSTRAINT `fk_formsubmission_assignment` FOREIGN KEY (`AssignmentID`) REFERENCES `teacher_assignment` (`AssignmentID`) ON DELETE SET NULL ON UPDATE CASCADE,
  ADD CONSTRAINT `formsubmission_ibfk_1` FOREIGN KEY (`StudentIDNumber`) REFERENCES `student` (`IDNumber`),
  ADD CONSTRAINT `formsubmission_ibfk_2` FOREIGN KEY (`EvaluationID`) REFERENCES `evaluationform` (`EvaluationID`),
  ADD CONSTRAINT `formsubmission_ibfk_3` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`);

--
-- Constraints for table `questionoption`
--
ALTER TABLE `questionoption`
  ADD CONSTRAINT `questionoption_ibfk_1` FOREIGN KEY (`QuestionID`) REFERENCES `surveyquestion` (`QuestionID`) ON DELETE CASCADE;

--
-- Constraints for table `report`
--
ALTER TABLE `report`
  ADD CONSTRAINT `fk_report_assignment` FOREIGN KEY (`AssignmentID`) REFERENCES `teacher_assignment` (`AssignmentID`) ON DELETE SET NULL,
  ADD CONSTRAINT `report_ibfk_1` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`),
  ADD CONSTRAINT `report_ibfk_2` FOREIGN KEY (`EvaluationID`) REFERENCES `evaluationform` (`EvaluationID`);

--
-- Constraints for table `subject`
--
ALTER TABLE `subject`
  ADD CONSTRAINT `subject_ibfk_1` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`) ON DELETE SET NULL,
  ADD CONSTRAINT `subject_ibfk_2` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`) ON DELETE SET NULL;

--
-- Constraints for table `surveyquestion`
--
ALTER TABLE `surveyquestion`
  ADD CONSTRAINT `surveyquestion_ibfk_1` FOREIGN KEY (`EvaluationID`) REFERENCES `evaluationform` (`EvaluationID`) ON DELETE CASCADE;

--
-- Constraints for table `surveyresponse`
--
ALTER TABLE `surveyresponse`
  ADD CONSTRAINT `surveyresponse_ibfk_2` FOREIGN KEY (`QuestionID`) REFERENCES `surveyquestion` (`QuestionID`),
  ADD CONSTRAINT `surveyresponse_ibfk_3` FOREIGN KEY (`SubmissionID`) REFERENCES `formsubmission` (`SubmissionID`) ON DELETE CASCADE;

--
-- Constraints for table `teacher_assignment`
--
ALTER TABLE `teacher_assignment`
  ADD CONSTRAINT `teacher_assignment_ibfk_1` FOREIGN KEY (`TeacherID`) REFERENCES `teacher` (`TeacherID`) ON DELETE CASCADE;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
