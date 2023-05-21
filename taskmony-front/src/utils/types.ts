export type TTask = {
  id: string;
  groupId: string;
  description: string;
  completedAt: string | null;
  deletedAt: string | null;
  assignee: {
    displayName: string;
    id: string;
  };
  subscribers: Array<{
    id: string;
  }>;
  details?: string | null;
  startAt: string;
  direction: { name: string; id: string };
  repeatMode: string | null;
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
    deletedAt: string | null;
    reviewedAt: string | null;
    subscribers: 
    Array<{
        id: string;
    }>;
    details?: string | null;
    direction: 
    { name: string; 
      id: string;
     }
    generation: string;
    createdBy: { 
      id: string;
      displayName: string;
     }
    comments: Array<TComment>
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
  type: string;
  direction: {id: string, name: string};
  name: string;
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

export type TUser = {
  displayName: string;
  id: string;
}

