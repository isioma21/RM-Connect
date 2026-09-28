import { Typography } from '@mui/material'
import { useAuth } from '../../auth/AuthContext'

export function ManagerHomePage() {
  const { user } = useAuth()

  return (
    <>
      <Typography variant="h1">Welcome, {user?.firstName}</Typography>
      <Typography color="text.secondary">Your customer requests and schedule will show here.</Typography>
    </>
  )
}
