import { Stack, Typography } from '@mui/material'
import { Greeting } from '../../components/Greeting'

export function ManagerHomePage() {
  return (
    <Stack spacing={1}>
      <Greeting />
      <Typography color="text.secondary">Your customer requests and schedule will show here.</Typography>
    </Stack>
  )
}
