import { Typography } from '@mui/material'
import { useAuth } from '../../auth/AuthContext'

export function CustomerHomePage() {
  const { user } = useAuth()

  return (
    <>
      <Typography variant="h1">Welcome, {user?.firstName}</Typography>
      <Typography color="text.secondary">Your relationship manager and appointments will show here.</Typography>
    </>
  )
}
