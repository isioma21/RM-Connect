import { useState } from 'react'
import { Typography } from '@mui/material'
import { useAuth } from '../auth/AuthContext'

function timeOfDay(hour: number) {
  if (hour < 12) return 'Good morning'
  if (hour < 17) return 'Good afternoon'
  return 'Good evening'
}

/** "Good morning, Chidi", based on the time on the user's device. */
export function Greeting() {
  const { user } = useAuth()
  const [greeting] = useState(() => timeOfDay(new Date().getHours()))

  return (
    <Typography variant="h3" component="p" color="text.secondary">
      {greeting}, {user?.firstName}
    </Typography>
  )
}
