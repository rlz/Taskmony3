export const tasksAllQuery = `id
    description
    completedAt
    deletedAt
    assignee
    {
      displayName
      id
    }
    subscribers 
    {
        id
    }
    details
    startAt
    direction 
    { name 
      id
     }
    repeatMode
    createdBy { displayName }
    comments {
     text
     createdAt
     createdBy { displayName } 
    }
    repeatMode
    repeatUntil
    weekDays`;

export const directionsAllQuery = `{directions{
    id
    name
    details
    members
    {
    displayName
    id
    }
    deletedAt
  }}`;

export const userAllQuery = `{users(){
    displayName
    email
  }}`;
