export const tasksAllQuery = `{tasks{
    id
    description
    completedAt
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
  }}`

  export const directionsAllQuery = `{directions{
    id
    name
    members
    {
    displayName
    id
    }
  }}`