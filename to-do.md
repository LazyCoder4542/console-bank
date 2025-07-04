1. If writing to file fails, reset the writer's position and don't update index,
2. nullable fields, idea: storing as [null bit][value]. 0 for non-null, 1 for null.
3. Deleting record, idea: remove record from index file and then rebuilding data file on DB init and also compacting index to reduce overhead after startup