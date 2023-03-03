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
    oldValue`

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
