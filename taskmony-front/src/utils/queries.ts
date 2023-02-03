export const taskAllQuery = `{tasks{
    id
    description
    details
    startAt
    direction { name }
    repeatMode
    createdBy { displayName }
  }}`