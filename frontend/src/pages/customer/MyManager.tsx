import { useState } from 'react'
import { Box, Button, Card, CardContent, Stack, Typography } from '@mui/material'
import { Link as RouterLink } from 'react-router-dom'
import { relationshipsApi } from '../../api/relationships'
import type { Relationship } from '../../api/types'
import { ErrorAlert } from '../../components/ErrorAlert'
import { ReasonDialog } from '../../components/ReasonDialog'
import { StatusChip } from '../../components/StatusChip'
import { UserAvatar } from '../../components/UserAvatar'
import { formatDay } from '../../utils/format'
import { MyAppointments } from './MyAppointments'

type Props = { relationship: Relationship; onChanged: () => void }

export function MyManager({ relationship, onChanged }: Props) {
  const { manager } = relationship
  const [error, setError] = useState<unknown>(null)
  const [confirming, setConfirming] = useState(false)

  async function endRelationship(reason: string) {
    setConfirming(false)
    try {
      await relationshipsApi.end(relationship.id, reason)
      onChanged()
    } catch (err) {
      setError(err)
    }
  }

  return (
    <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', md: '380px 1fr' }, gap: 4, alignItems: 'start' }}>
      <Stack spacing={2}>
        <Typography variant="h2" component="h1">My relationship manager</Typography>
        <ErrorAlert error={error} />
        <Card>
          <CardContent sx={{ p: 3.5 }}>
            <Stack spacing={2.5}>
              <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'flex-start' }}>
                <UserAvatar user={manager} size={64} />
                <StatusChip status="Active" />
              </Stack>
              <Box>
                <Typography sx={{ fontSize: 22, fontWeight: 700 }}>{manager.firstName} {manager.lastName}</Typography>
                <Typography color="text.secondary">{manager.branch} branch</Typography>
                <Typography color="text.secondary">{manager.email}</Typography>
              </Box>
              {relationship.respondedAt && (
                <Typography variant="body2" color="text.secondary">
                  Your manager since {formatDay(relationship.respondedAt)}
                </Typography>
              )}
              <Button variant="contained" size="large" component={RouterLink} to="/customer/book">
                Book an appointment
              </Button>
              <Button color="error" onClick={() => setConfirming(true)}>End relationship</Button>
            </Stack>
          </CardContent>
        </Card>
      </Stack>

      <MyAppointments />

      {confirming && (
        <ReasonDialog
          title="End relationship?"
          message={`Are you sure you want to end your relationship with ${manager.firstName}? Your booked appointments stay as they are.`}
          label="Why are you leaving? (optional)"
          helperText="Your feedback helps us improve."
          confirmLabel="Yes, end relationship"
          onConfirm={endRelationship}
          onClose={() => setConfirming(false)}
        />
      )}
    </Box>
  )
}
