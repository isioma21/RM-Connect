import { useState } from 'react'
import { Stack, Typography } from '@mui/material'
import { ChangePassword } from './ChangePassword'
import { Devices } from './Devices'

export function SecurityPage() {
  // Changing the password signs out other devices, so the list reloads after it
  const [devicesVersion, setDevicesVersion] = useState(0)

  return (
    <Stack spacing={4} sx={{ maxWidth: 760 }}>
      <Typography variant="h1">Security</Typography>
      <ChangePassword onChanged={() => setDevicesVersion(devicesVersion + 1)} />
      <Devices key={devicesVersion} />
    </Stack>
  )
}
