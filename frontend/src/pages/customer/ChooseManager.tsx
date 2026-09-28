import { useEffect, useState } from 'react'
import { Box, Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { relationshipsApi } from '../../api/relationships'
import type { User } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { UserAvatar } from '../../components/UserAvatar'

export function ChooseManager({ onChanged }: { onChanged: () => void }) {
  const [managers, setManagers] = useState<User[]>([])
  const [error, setError] = useState<unknown>(null)

  useEffect(() => {
    relationshipsApi.managers().then(setManagers).catch(setError)
  }, [])

  async function request(manager: User) {
    try {
      await relationshipsApi.request(manager.id)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <Stack spacing={3}>
      <Box>
        <Typography variant="h1">Choose your relationship manager</Typography>
        <Typography color="text.secondary">
          Send a request to one manager. Once they accept, you can book calls and branch visits with them.
        </Typography>
      </Box>
      <ErrorAlert error={error} />
      <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: 'repeat(3, 1fr)' }, gap: 3 }}>
        {managers.map((manager) => (
          <Card key={manager.id}>
            <CardContent sx={{ p: 3.5 }}>
              <Stack spacing={2.5}>
                <Stack direction="row" spacing={2} sx={{ alignItems: 'center' }}>
                  <UserAvatar user={manager} />
                  <Box>
                    <Typography sx={{ fontSize: 18, fontWeight: 700 }}>{manager.firstName} {manager.lastName}</Typography>
                    <Typography color="text.secondary">{manager.branch} branch</Typography>
                  </Box>
                </Stack>
                <Typography variant="body2" color="text.secondary">{manager.email}</Typography>
                <Button variant="contained" onClick={() => request(manager)}>Request {manager.firstName}</Button>
              </Stack>
            </CardContent>
          </Card>
        ))}
      </Box>
    </Stack>
  )
}
