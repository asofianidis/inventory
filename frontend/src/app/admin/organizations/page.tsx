import {
  Table,
  TableBody,
  TableCaption,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";
import { Organizations } from "@/types/organizations";
import ModifyOrganization from "./modify-organization";

async function GetOrganizations(): Promise<Organizations[]> {
  try {
    const res = await fetch(
      `${process.env.NEXT_PUBLIC_API_URL}/api/Organizations/GetAllOrgs`,
      { cache: "no-cache" }
    );

    if (!res.ok) {
      throw new Error(
        `Failed to fetch organizations, status code ${res.status}`
      );
    }

    const data = await res.json();

    return Array.isArray(data) ? data : [];
  } catch (e: any) {
    console.log(e);
    return [];
  }
}

export default async function OrganizationPage() {
  const organizations = await GetOrganizations();

  return (
    <div className="mt-8 mx-6 mb-6 flex flex-col gap-5">
      <h2 className="text-2xl font-bold">Organizations</h2>
      <div className="flex justify-end">
        <ModifyOrganization />
      </div>
      <hr />
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead className="font-bold text-black">ID</TableHead>
            <TableHead>Name</TableHead>
            <TableHead></TableHead>
          </TableRow>
        </TableHeader>
        <TableBody>
          {organizations.map((o) => {
            return (
              <TableRow>
                <TableCell>{o.org_id}</TableCell>
                <TableCell>{o.name}</TableCell>
                <TableCell>
                  <ModifyOrganization
                    organization={o}
                    key={o.org_id.toString()}
                  />
                </TableCell>
              </TableRow>
            );
          })}
        </TableBody>
      </Table>
    </div>
  );
}
