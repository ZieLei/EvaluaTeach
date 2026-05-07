# Fix Multiple Choice Options Not Saving

The multiple choice question options are not being persisted to the database. The `QuestionOption` table exists but the code never inserts/reads from it.

## Plan

1. **Modify `FormDataStore.AddForm()`** (lines 156-170):
   - After inserting each question, also insert its options into `QuestionOption` table
   - Only do this for questions that have Options (MultipleChoice type)

2. **Modify `FormDataStore.GetQuestionsForForm()`** (lines 105-130):
   - After loading each question, also load its options from `QuestionOption` table
   - Populate `question.Options` list with the option text values

## Code Changes

### AddForm - Add after line 169 (after q.Id is set):
```csharp
// Insert options for multiple choice questions
if (q.Options != null && q.Options.Count > 0)
{
    foreach (var optionText in q.Options)
    {
        var optCmd = new MySqlCommand(@"
            INSERT INTO QuestionOption (QuestionID, OptionText, OrderIndex)
            VALUES (@questionId, @optionText, @orderIdx)", conn, tx);
        optCmd.Parameters.AddWithValue("@questionId", q.Id);
        optCmd.Parameters.AddWithValue("@optionText", optionText);
        optCmd.Parameters.AddWithValue("@orderIdx", q.Options.IndexOf(optionText));
        optCmd.ExecuteNonQuery();
    }
}
```

### GetQuestionsForForm - Add after line 128 (inside the while loop, before returning):
```csharp
// Load options for this question
question.Options = GetOptionsForQuestion(question.Id, conn);
```

Add new helper method after GetQuestionsForForm:
```csharp
private static List<string> GetOptionsForQuestion(int questionId, MySqlConnection conn)
{
    var options = new List<string>();
    var cmd = new MySqlCommand(@"
        SELECT OptionText FROM QuestionOption 
        WHERE QuestionID = @questionId 
        ORDER BY OrderIndex", conn);
    cmd.Parameters.AddWithValue("@questionId", questionId);
    
    using var reader = cmd.ExecuteReader();
    while (reader.Read())
    {
        options.Add(reader.GetString("OptionText"));
    }
    return options;
}
```
