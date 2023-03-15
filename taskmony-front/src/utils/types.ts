export type TTask = {
  id: string;
  description: string;
  completedAt: string;
  deletedAt: string;
  assignee: {
    displayName: string;
    id: string;
  };
  subscribers: Array<{
    id: string;
  }>;
  details?: string;
  startAt: string;
  direction: { name: string; id: string };
  repeatMode: string;
  createdBy: {
    id: string;
    displayName: string;
  };
  repeatEvery: number;
  comments: Array<TComment>;
  repeatUntil: string;
  weekDays: Array<String>;
};

export type TIdea = {
  id: string;
    description: string;
    deletedAt: string;
    reviewedAt: string;
    subscribers: 
    Array<{
        id: string;
    }>;
    details?: string;
    direction: 
    { name: string; 
      id: string;
     }
    generation: string;
    createdBy: { 
      id: string;
      displayName: string;
     }
    comments: Array<{
     text: string;
     createdAt: string;
     createdBy: { displayName: string; } 
    }>
}
export type TDirection = {
  id: string;
  name: string;
  details: string;
  members:
  Array<{
  displayName: string;
  id: string;
  }>
  deletedAt: string;
};

export type TComment = {
  text: string;
  createdAt: string;
  createdBy: { displayName: string };
}
export type TNotification = {
  actionItem: {
    __typename: string;
    id: string;
    displayName?: string;
    description?: string;
    details?: string;
    text?: string;
    }
  actionType: string;
  field: string;
  id: string;
  modifiedAt: string;
  modifiedBy: { displayName: string; };
  newValue: string;
  oldValue: string; 
};
