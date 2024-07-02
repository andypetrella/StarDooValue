# Quest 1: Understanding Trust Loss

## Objective
Talk to villagers to understand the trust issues with data and investigate the pipelines to identify root causes.

## Steps

### Scene 1: Receiving Mail from Lewis

**Mail Content**:
Dear @,

I have an urgent matter to discuss with you regarding the town’s data management. Please visit me in my office at your earliest convenience.

Best regards,
Mayor Lewis

### Scene 2: Dialogue with Lewis

**Lewis**: "Ah, @! I'm glad you came. Our town relies heavily on data-driven decisions, but recently, there have been inconsistencies that I can't explain. I need your help to investigate and resolve these issues."

**Player**: "What do you need me to do?"

**Lewis**: "I need you to talk to some key villagers and gather their feedback on the data issues. This will help us understand the problem better and work towards a solution. Will you help us?"

### Scene 3: Gathering Feedback from Villagers

**Robin**: "I've noticed that some of the construction data seems off. Orders get mixed up, and sometimes materials are missing. It's frustrating when you have 10 logs one day, but only 5 the next, even though I didn't use any!"

**Pierre**: "Sales reports are not matching up with the inventory. It's causing confusion and affecting my business decisions. For example, last week's report showed 20 units of fertilizer sold, but only 10 were recorded in the inventory."

**Marnie**: "Some of the livestock data seems incorrect. It's affecting my ability to manage the farm efficiently. For instance, one report showed I had 8 cows, but I only have 6."

**Gus**: "The ingredient orders and inventory levels are not matching up. It's creating problems in running the saloon smoothly. I had an order for 30 tomatoes, but only 20 were delivered, causing shortages."

### Scene 4: Investigating Pipelines

**Mine**:
- **Player**: "You find data logs showing inconsistencies in the extracted data."
- **Dialogue**: "The mining data shows discrepancies in the quantity and quality of extracted materials. For example, the logs indicate 100 units of ore were mined, but only 70 were recorded."

**Blacksmith**:
- **Player**: "Clint provides insights into the issues with processing raw data."
- **Clint**: "We've noticed some irregularities in the data we're processing. It seems like some data is getting corrupted. For instance, the data logs for processed iron are incomplete, showing gaps for certain days."

### Scene 5: Discussing with Data Engineers and Quality Analysts

**Clint**: "We've noticed some irregularities in the data we're processing. It seems like some data is getting corrupted. For instance, the data logs for processed iron are incomplete, showing gaps for certain days."

**Maru**: "There are gaps in the data logs, making it hard to track where things go wrong. For example, some logs show data up to 2 PM, then nothing until the next day."

**Emily**: "The quality of the data is poor. It’s affecting our ability to make informed decisions. Sometimes the data is just wrong, like a report showing a 50% increase in sales that never happened."

**Leah**: "We need better checks and controls to ensure data integrity. For example, a recent report showed all products were in stock, but we were actually out of several key items."

### Scene 6: Identifying Root Causes

**Player**: Synthesizes information from investigations and discussions.

**Dialogue**: "You identify several root causes, including data corruption, gaps in data logs, and poor data quality controls. For example, corrupted data logs at the blacksmith, missing data entries at the mine, and incorrect inventory reports at JojaMart."

### Scene 7: Reporting Back to Lewis

**Player**: "I’ve gathered feedback from the villagers and investigated the pipelines. Here’s what I’ve found."

**Lewis**: "Thank you for gathering this information. It’s clear that we have some data management issues to address. Let’s work together to improve the situation."

**Player**: "Implementing data observability can help us monitor and resolve these issues more effectively. For example, by filling data gaps, identifying rules for data integrity, and anticipating detection of issues before they propagate and lead to bad decisions."

**Lewis**: "That sounds like a solid plan. Let me reflect on this."

### Scene 8: Receiving Mail from Lewis

> Transition to Quest 2

**Mail Content**:
Dear @,

I've been thinking about our discussion and the situation worries me. Your proposal feels ambitious but promising. I think we should experiment with it on a specific scope to see how it works. Can you propose a use case that would demonstrate the system in a non-trivial way?

Best regards,
Mayor Lewis

### Scene 9: Dialogue with Lewis

**Player**: "I propose we start with Maru’s case. She’s been dealing with gaps in the data logs, which make it hard to track where things go wrong."

**Lewis**: "Alright, let’s give it a try. Show me how data observability can help in this situation."

**Player**: "I’ll get started on setting up the system and gathering initial data."