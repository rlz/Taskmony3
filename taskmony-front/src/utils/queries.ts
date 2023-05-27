export const tasksAllQuery = `id
    groupId
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
    createdBy { 
      id
      displayName 
    }
    comments {
     text
     createdAt
     createdBy { displayName } 
    }
    repeatEvery
    repeatUntil
    weekDays`;

export const ideasAllQuery = `id
    description
    deletedAt
    reviewedAt
    subscribers 
    {
        id
    }
    details
    direction 
    { name 
      id
     }
    generation
    createdBy { 
      id
      displayName
     }
    comments {
     text
     createdAt
     createdBy { displayName } 
    }`;

export const notificationsAllQuery = `
    actionItem {
      __typename
      ... on User {
        id
        displayName
      }
      ... on Task {
        id
        description
        details
      }
      ... on Idea {
        id
        description
        details
      }
      ... on Comment {
        id
        text
      }

    }
    actionType
    field
    id
    modifiedAt
    modifiedBy { displayName }
    newValue
    oldValue`;

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
