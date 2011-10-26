ALTER TABLE UserEvent Drop CONSTRAINT FK_UserEvent_Event;
ALTER TABLE UserEvent Add constraint FK_UserEvent_Event foreign key (EventId) references Event(EventId) ON DELETE CASCADE;

ALTER TABLE UserEvent Drop CONSTRAINT FK_Winner_Event;
ALTER TABLE UserEvent Add constraint FK_Winner_Event foreign key (EventId) references Event(EventId) ON DELETE CASCADE;