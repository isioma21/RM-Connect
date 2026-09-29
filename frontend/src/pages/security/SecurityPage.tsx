import { Stack, Typography } from '@mui/material'
import { ChangePassword } from './ChangePassword'
import { Devices } from './Devices'

export function SecurityPage() {
  return (
    <Stack spacing={4} sx={{ maxWidth: 760 }}>
      <Typography variant="h1">Security</Typography>
      <ChangePassword />
      <Devices />
    </Stack>
  )
}
