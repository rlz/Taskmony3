export type TTask = {
  description: string;
  details: string;
  assigneeId: string;
  directionId: string;
  startAt: string;
  id: string;
  comments: Array<TComment>;
};
export type TDirection = {
  name: string;
  details: string;
  deletedAt: string;
  members: Array<{ displayName: string; id: string }>;
  id: string;
};

export type TComment = {
  text: string;
  createdAt: string;
  createdBy: { displayName: string };
};


